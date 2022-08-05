import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-access',
  templateUrl: './access.component.html',
  styleUrls: ['./access.component.css']
})
export class AccessComponent implements OnInit {

  isProcessing: boolean = false;
  accesses: Access[] = [];

  constructor(private http: HttpClient) {
    this.loadAll();
  }

  ngOnInit(): void {
  }

  loadAll() {
    this.isProcessing = true;
    this.http.get<Access[]>('/api/access').subscribe(result => {
      this.accesses = result;
      this.isProcessing = false;
    }, error => {
      this.isProcessing = false;
      console.error(error);
    });
  }

}

interface Access {
  id: number;
  correlationId: string;
  username: string;
  action: string;
  timeStamp: string;
  result: string;
}
