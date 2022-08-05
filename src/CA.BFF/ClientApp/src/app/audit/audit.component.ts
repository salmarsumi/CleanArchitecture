import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-audit',
  templateUrl: './audit.component.html',
  styleUrls: ['./audit.component.css']
})
export class AuditComponent implements OnInit {
  isProcessing: boolean = false;
  audits: Audit[] = [];

  constructor(private http: HttpClient) {
    this.loadAll();
  }

  ngOnInit(): void {
  }

  loadAll() {
    this.isProcessing = true;
    this.http.get<Audit[]>('/api/audit').subscribe(result => {
      this.audits = result;
      this.isProcessing = false;
    }, error => {
      this.isProcessing = false;
      console.error(error);
    });
  }
}

interface Audit {
  id: number;
  correlationId: string;
  username: string;
  action: string;
  timeStamp: string;
  source: string;
  object: string;
}
