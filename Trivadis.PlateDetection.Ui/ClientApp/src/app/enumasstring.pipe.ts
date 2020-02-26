import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'EnumAsString'
})

export class EnumAsStringPipe implements PipeTransform {
  transform(value: number, enumType: any): any {
    return enumType[value].split(/(?=[A-Z])/).join().replace(",", " ");
  }
}
