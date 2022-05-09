import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient) {
    this.loadAll();
  }

  addNew() {
    this.http.post('/api/weather', {}).subscribe(result => {
      this.loadAll();

    }, error => console.error(error));
  }

  loadAll() {
    this.http.get<WeatherForecast[]>('/api/weather').subscribe(result => {
      this.forecasts = result;

    }, error => console.error(error));
  }

  delete(id: number) {
    this.http.delete(`/api/weather/${id}`).subscribe(result => {
      this.loadAll();
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  id: number;
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
