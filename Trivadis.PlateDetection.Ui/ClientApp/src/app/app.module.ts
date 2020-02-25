import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { JobListComponent } from './job-list/job-list.component';
import { JobDetailComponent } from './job-detail/job-detail.component';
import { ResultListComponent } from './result-list/result-list.component';
import { ResultDetailComponent } from './result-detail/result-detail.component';
import { PlateListComponent } from './plate-list/plate-list.component';
import { PlateDetailComponent } from './plate-detail/plate-detail.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    JobListComponent,
    JobDetailComponent,
    ResultListComponent,
    ResultDetailComponent,
    PlateListComponent,
    PlateDetailComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'jobs', component: JobListComponent },
      { path: 'job/:id', component: JobDetailComponent },      
      { path: 'results', component: ResultListComponent },
      { path: 'plates', component: PlateListComponent },      
    ], { enableTracing: true })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
