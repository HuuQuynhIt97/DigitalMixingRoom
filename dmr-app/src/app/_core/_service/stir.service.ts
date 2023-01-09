import { Injectable, OnDestroy } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { IStir, IStirForAdd, IStirForUpdate } from '../_model/stir';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: 'Bearer ' + localStorage.getItem('token')
  })
};
@Injectable({
  providedIn: 'root'
})
export class StirService implements OnDestroy {
  baseUrl = environment.apiUrlEC;
  baseUrlLocal = environment.apiLocal;
  numberOfAttempts: number;
  public hubConnection: HubConnection;
  private connectionUrl = environment.rpmHub;
  private counter = 0;
  receiveAmount: BehaviorSubject<{ stirScaleID: any ,rpm: any }>;
  constructor(private http: HttpClient) { 
    this.counter++;
    console.log(this.counter);
    this.numberOfAttempts = 0;
    this.receiveAmount = new BehaviorSubject<{ stirScaleID: any, rpm: any}>(null);
  }
  ngOnDestroy() {
    console.log('Stir Service destroy');
    this.numberOfAttempts = 0;
    
  }

  public connect = () => {
    this.startConnection();
  }
  public close = async () => {
    try {
      await this.hubConnection.stop();
      console.log('Stir service stoped hub');
    } catch (error) {
      console.log('Stir service Cant not stop hub', error);
    }
  }

  startConnection = async () => {
    this.numberOfAttempts = this.numberOfAttempts + 1;
    this.hubConnection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Error)
      .withUrl(this.connectionUrl)
      .build();
    this.setSignalrClientMethods();
    try {
      await this.hubConnection.start();

      console.log('StirService connected hub');

    } catch (error) {
      alert('Không thể kết nối tới signal RPM! Vui lòng liên hệ administrator!');
      // setTimeout(async () => {
      //   if (this.numberOfAttempts === 5) {
      //     this.numberOfAttempts = 0;
      //     console.log(`mixing service cant not connected hub: ${error}`, this.numberOfAttempts);
      //     alert('Đã kết nối lại cân 5 lần nhưng không được! Vui lòng liên hệ administrator!');
      //     return;
      //   }
      //   await this.startConnection();
      // }, 5000);
    }
  }
  public offWeighingScale() {
    this.hubConnection.off('Welcome');
  }
  // This method will implement the methods defined in the ISignalrDemoHub inteface in the SignalrDemo.Server .NET solution
  private setSignalrClientMethods(): void {
    this.hubConnection.onreconnected(() => {
      console.log('Stir service Restarted signalr!');
    });

    this.hubConnection.on('Welcome', (stirScaleID, rpm) => {
      this.receiveAmount.next({stirScaleID, rpm });
    });

    this.hubConnection.on('UserConnected', (conId) => {
      console.log('Stir service', conId);
    });

    this.hubConnection.on('UserDisconnected', (conId) => {
      console.log('Stir service UserDisconnected', conId);

    });

  }
  getStirInfo(glueName) {
    return this.http.get(`${this.baseUrl}Stir/GetStirInfo/${glueName}`);
  }

  getRPM(stirID) {
    return this.http.get(`${this.baseUrl}Stir/GetRPM/${stirID}`);
  }

  getRPMByMachineCode(machineCode, startTime, endTime) {
    return this.http.get(`${this.baseUrl}Stir/GetRPMByMachineCode/${machineCode}/${startTime}/${endTime}`);
  }

  getStirByMixingInfoID(mixingInfoID: number) {
    return this.http.get<IStir[]>(`${this.baseUrl}Stir/getStirByMixingInfoID/${mixingInfoID}`);
  }

  create(model: IStirForAdd) {
    return this.http.post(`${this.baseUrl}Stir/Create`, model);
  }

  createLocal(model: IStirForAdd) {
    return this.http.post(`${this.baseUrlLocal}Stir/Create`, model);
  }

  update(model: IStirForUpdate) {
    return this.http.put(`${this.baseUrl}Stir/Update`, model);
  }

  updateLocal(model: IStirForUpdate) {
    return this.http.put(`${this.baseUrlLocal}Stir/Update`, model);
  }

  scanMachine(buildingID: number, scanValue: string) {
    return this.http.get(`${this.baseUrl}Stir/scanMachine/${buildingID}/${scanValue}`, {} );
  }

  updateStartScanTime(mixingInfoID: number) {
    return this.http.put(`${this.baseUrl}Stir/UpdateStartScanTime/${mixingInfoID}`, {});
  }

}
