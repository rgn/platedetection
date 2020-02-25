using System;
using System.Linq;
using System.IO;
using openalprnet;
using System.ComponentModel;
using System.Collections.Generic;
using Serilog;
using Trivadis.PlateDetection.Model;
using Microsoft.Extensions.Options;
using Trivadis.PlateDetection.Broker;

namespace Trivadis.PlateDetection
{
    public class PlateDetectionService
    {
        private string plateDetectionBasePath;
        private string region;
        private string pathToAlprConf;
        private string pathToRuntimeData;

        private readonly ApplicationDatabaseContext dbContext;
        private readonly KeylessBrokerProducerService<Job>  brokerProducerService;
        private AlprNet alpr;
        private readonly ILogger logger;
        private string plateDectectionIncomingPath => Path.Combine(plateDetectionBasePath, "incoming");
        private string plateDectectionProcessedPath => Path.Combine(plateDetectionBasePath, "processed");
        private string plateDectectionErrorPath => Path.Combine(plateDetectionBasePath, "error");
        private List<string> validExtensions = new List<string>
        {
            ".png",
            ".jpg",
            ".gif",
            ".bmp",
            ".tif"
        };

        public PlateDetectionService(IOptions<PlateDetectionServiceOptions> options, ApplicationDatabaseContext dbContext, KeylessBrokerProducerService<Job> brokerProducer, Serilog.ILogger logger)
        {
            region = options.Value.AlprRegion;
            pathToAlprConf = options.Value.AlprConfigFile;
            pathToRuntimeData = options.Value.AlprRuntimeData;
            plateDetectionBasePath = options.Value.PlateDetectionPath;

            this.dbContext = dbContext;
            brokerProducerService = brokerProducer;
            this.logger = logger;

            logger.Information("PlateDetectionWorker initialized.");
        }

        public void Start()
        {
            logger.Information("PlateDetectionWorker starting...");

            if (!Directory.Exists(plateDetectionBasePath)) Directory.CreateDirectory(plateDetectionBasePath);
            if (!Directory.Exists(plateDectectionIncomingPath)) Directory.CreateDirectory(plateDectectionIncomingPath);
            if (!Directory.Exists(plateDectectionProcessedPath)) Directory.CreateDirectory(plateDectectionProcessedPath);
            if (!Directory.Exists(plateDectectionErrorPath)) Directory.CreateDirectory(plateDectectionErrorPath);

            if (!File.Exists(pathToAlprConf)) throw new Exception("ALPR configuration file doesn't exist.");
            if (!Directory.Exists(pathToRuntimeData)) throw new Exception("Path to ALPR runtime data doesn't exist.");

            alpr = new AlprNet(region, pathToAlprConf, pathToRuntimeData);

            alpr.FrameProcessed += Alpr_FrameProcessed;

            if (!alpr.IsLoaded())
            {
                logger.Error("OpenAlpr failed to load!");                               

                throw new Exception("OpenAlpr failed to load!");
            }
            
            //alpr.Configuration.DebugPlateCorners = true;
            //alpr.Configuration.DebugPlateLines = true;

            alpr.DetectRegion = true;

            var existingFiles = Directory.GetFiles(plateDectectionIncomingPath);

            if (existingFiles.Any())
            {
                logger.Information($"Found {existingFiles.Length} existing files.");

                var bw = new BackgroundWorker();
                bw.DoWork += Bw_DoWork;
                bw.RunWorkerAsync(existingFiles);
            }

            var fsw = new FileSystemWatcher(plateDectectionIncomingPath, "*.*");
            fsw.Created += Fsw_Created;
            fsw.EnableRaisingEvents = true;

            logger.Information("PlateDetectionWorker started...");
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var existingFiles = e.Argument as string[];
            var jobs = new List<Job>();

            foreach (var existingFile in existingFiles)
            {
                if (File.Exists(existingFile))
                {
                    var job = ProcessImage(existingFile);
                    brokerProducerService.SendAsync(null, job);
                    jobs.Add(ProcessImage(existingFile));
                }
            }

            dbContext.AddRange(jobs);
            dbContext.SaveChangesAsync();
        }

        private void Alpr_FrameProcessed(object sender, AlprFrameEventArgs e)
        {
            logger.Information("PlateDetectionWorker FrameProcessed...");
        }

        private void Fsw_Created(object sender, FileSystemEventArgs e)
        {
            logger.Information($"File {e.FullPath} created...");

            var job = ProcessImage(e.FullPath);

            brokerProducerService.SendAsync(null, job);
            dbContext.Add(job);
            dbContext.SaveChangesAsync();
        }

        public void Stop()
        {
            logger.Information("PlateDetectionWorker stopping...");

            logger.Information("PlateDetectionWorker stopped...");
        }

        private Job ProcessImage(string fileName)
        {
            AlprResultsNet result;
            Job job = new Job();

            var extension = Path.GetExtension(fileName);
            if (!validExtensions.Contains(extension))
            {
                logger.Information($"Invalid file extension of file {fileName}, moving to error directory.");

                MoveFile(fileName, plateDectectionErrorPath, true);
            }

            logger.Information($"Processing file {fileName}");

            try
            {
                result = alpr.Recognize(fileName);

                job = new Job
                {
                    FileName = fileName,
                    ImageHeight = result.ImageHeight,
                    ImageWidth = result.ImageWidth,
                    Result = result.Json,
                    TotalProcessingTimeInMs = result.TotalProcessingTimeMs,
                    ImageData = File.ReadAllBytes(fileName),
                    Rectangles = result.RegionsOfInterest.Select(r => new Rectangle { X = r.X, Y = r.X, Width = r.Width, Height = r.Height }).ToList(),
                    State = JobState.Succeeded,
                    DetectionResults = result.Plates.Select(dp => new DetectionResult
                    {                           
                        Region = dp.Region,
                        RegionConfidence = dp.RegionConfidence,
                        RequestedTopN = dp.RequestedTopN,
                        ProcessingTimeInMs = dp.ProcessingTimeMs,                        
                        DetectedPoints = dp.PlatePoints.Select(p => new DetectedPoint { X = p.X, Y = p.Y }).ToList(),
                        DetectedPlates = dp.TopNPlates.Select(x => new DetectedPlate
                        {
                            OverallConfidence = x.OverallConfidence,
                            MatchesTemplate = x.MatchesTemplate,
                            Characters = x.Characters
                        }).ToList()
                    }).ToList()
                };

                MoveFile(fileName, plateDectectionProcessedPath, true);

                logger.Information($"Processed file {fileName}");
            }
            catch (Exception ex)
            {
                job = new Job
                {
                    FileName = fileName,
                    ImageData = File.Exists(fileName) ? File.ReadAllBytes(fileName) : null,
                    State = JobState.Failed
                };

                logger.Error(ex, $"Failed to process file {fileName}");
            }

            return job;
        }

        private void MoveFile(string inptutFileName, string outputDirectory, bool useDate = true)
        {
            var outputPath = outputDirectory;

            if (useDate)
            {
                outputPath = Path.Combine(outputDirectory, DateTime.Today.ToString("yyyy-MM-dd/hhmm"));
            }

            if (!Directory.Exists(outputPath))
            {
                try
                {
                    Directory.CreateDirectory(outputPath);

                    logger.Information($"Output path {outputPath} created.");
                }
                catch
                {
                    logger.Error($"Failed to create output path {outputPath}");
                    return;
                }
            }

            var outputFile = Path.Combine(outputPath, Path.GetFileName(inptutFileName));
            try
            {
                File.Move(inptutFileName, outputFile);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to move file to {outputFile}.");
            }
        }
    }
}
