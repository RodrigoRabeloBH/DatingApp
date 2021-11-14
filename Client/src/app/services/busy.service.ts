import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount = 0;

  constructor(private service: NgxSpinnerService) { }

  busy() {
    this.busyRequestCount++;
    this.service.show(undefined, {
      type: 'ball-climbing-dot',
      bdColor: 'rgba(51,51,51,0.8)',
      color: '#fff'
    });
  }

  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.service.hide();
    }
  }
}
