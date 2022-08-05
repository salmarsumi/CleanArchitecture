import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  isProcessing: boolean = false;
  forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient) {
    this.loadAll();
  }

  addNew() {
    this.isProcessing = true;
    this.http.post('/api/weather', {}).subscribe(result => {
      this.loadAll();

    }, error => {
      this.isProcessing = false;
      console.error(error);
    });
  }

  loadAll() {
    this.isProcessing = true;
    this.http.get<WeatherForecast[]>('/api/weather').subscribe(result => {
      this.forecasts = result;
      this.isProcessing = false;
    }, error => {
      this.isProcessing = false;
      console.error(error);
    });
  }

  delete(id: number) {
    this.isProcessing = true;
    this.http.delete(`/api/weather/${id}`).subscribe(result => {
      this.loadAll();
    }, error => {
      this.isProcessing = false;
      console.error(error);
    });
  }
}

interface WeatherForecast {
  id: number;
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
