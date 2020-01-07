import {Injectable} from '@angular/core';
import {baseUrl} from '../../helpers/url-constants';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {RetrospectiveService} from './retrospective.service';
import {Observable} from 'rxjs';
import {RetroColumn} from '../../models/RetroColumn';
import {RetroFamily} from '../../models/RetroFamily';

@Injectable({
  providedIn: 'root'
})
export class RetroFamilyService {

  private readonly baseUrlRetrospective: string = baseUrl + 'retrospectives/';

  private readonly baseUrlRetroColumn = baseUrl + 'retrofamilies/';

  private httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient, private retrospectiveService: RetrospectiveService) {
  }

  getRetroColumns(): Observable<RetroFamily[]> {
    return this.http.get<RetroFamily[]>(this.baseUrlRetroColumn, this.httpOptions);
  }

  getRetroColumn(id): Observable<RetroFamily> {
    return this.http.get<RetroFamily>(this.baseUrlRetroColumn + id, this.httpOptions);
  }

  createColumn(retroFamily: RetroFamily): Observable<RetroFamily> {
    if (this.retrospectiveService.getCurrentRetrospective()) {
      return this.http.post<RetroFamily>(this.baseUrlRetroColumn, retroFamily, this.httpOptions);
    }
  }

  updateColumn(retroFamily: RetroFamily) {
    return this.http.put<RetroColumn>(this.baseUrlRetroColumn, retroFamily, this.httpOptions);
  }

  removeColumn(retroFamilyId): Observable<RetroFamily> {
    if (this.retrospectiveService.getCurrentRetrospective()) {
      return this.http.delete<RetroFamily>(this.baseUrlRetroColumn + retroFamilyId, this.httpOptions);
    }
  }
}
