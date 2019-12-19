import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { baseUrl } from '../../helpers/url-constants';
import { MessageService } from './message.service';
import { RetroColumn } from '../../models/retroColumn';
import { RetrospectiveService } from './retrospective.service';

@Injectable({
  providedIn: 'root'
})
export class RetrocolumnService {
  private readonly baseUrlRetrospective: string = baseUrl + 'retrospectives/';

  private readonly baseUrlRetroColumn = baseUrl + 'retrocolumns/';

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient, private retrospectiveService: RetrospectiveService) {
  }

  getRetroColumns(): Observable<RetroColumn[]> {
    return this.http.get<RetroColumn[]>(this.baseUrlRetroColumn, this.httpOptions);
  }

  getRetroColumn(id): Observable<RetroColumn> {
    return this.http.get<RetroColumn>(this.baseUrlRetroColumn + id, this.httpOptions);
  }

  createColumn(title, retrospectiveId): Observable<RetroColumn> {
    if (this.retrospectiveService.getCurrentRetrospective()) {
      return this.http.post<RetroColumn>(this.baseUrlRetroColumn, {
        title: title,
        retrospectiveId: retrospectiveId
      }, this.httpOptions);
    }
  }

  updateColumn(retroColumnId: number, title: string) {
    if (this.retrospectiveService.getCurrentRetrospective()) {
      return this.http.put<RetroColumn>(this.baseUrlRetroColumn + retroColumnId, {
        title: title,
        id: retroColumnId
      }, this.httpOptions);
    }
  }

  removeColumn(columnId): Observable<RetroColumn> {
    if (this.retrospectiveService.getCurrentRetrospective()) {
      return this.http.delete<RetroColumn>(this.baseUrlRetroColumn + columnId, this.httpOptions);
    }
  }
}