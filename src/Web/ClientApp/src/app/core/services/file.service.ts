import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface FileItem {
  id: string;
  name: string;
  path: string;
  size: number;
  createdAt: string;
  modifiedAt: string;
  isDirectory: boolean;
  itemType: number; // 0 = File, 1 = Directory
  parentFolderId: string | null;
  storageFileName?: string;
  contentType?: string;
}

export interface FolderItem {
  id: string;
  name: string;
  path: string;
  size: number;
  createdAt: string;
  modifiedAt: string;
  isDirectory: boolean;
  itemType: number;
  parentFolderId: string | null;
}

export interface FolderContentsResult {
  folders: FolderItemResponse[];
  files: FileItem[];
}

export interface FolderItemResponse {
  id: string;
  name: string;
  parentFolderId: string | null;
  createdAt: string;
}

export interface CreateFolderRequest {
  name: string;
  parentFolderId: string | null;
}

export interface UpdateFolderRequest {
  name: string;
}

export interface FileUploadResult {
  id: string;
  fileName: string;
  size: number;
}

export interface SearchRequest {
  q: string;
  type?: string;
  page?: number;
  size?: number;
}

export interface SearchResult {
  query: string;
  type: string | null;
  page: number;
  size: number;
  totalResults: number;
  results: any[];
}

@Injectable({
  providedIn: 'root'
})
export class FileService {
  private apiUrl = 'http://localhost:5252/api/v1';

  constructor(private http: HttpClient, private authService: AuthService) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found in localStorage');
      return new HttpHeaders();
    }
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // File operations
  getFileById(id: string): Observable<FileItem> {
    return this.http.get<FileItem>(`${this.apiUrl}/file/${id}`, { headers: this.getHeaders() });
  }

  uploadFile(file: File, folderId?: string): Observable<FileUploadResult> {
    const formData = new FormData();
    formData.append('file', file);
    if (folderId) {
      formData.append('folderId', folderId);
    }
    return this.http.post<FileUploadResult>(`${this.apiUrl}/file/upload`, formData, { headers: this.getHeaders() });
  }

  downloadFile(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/file/${id}/download`, { 
      headers: this.getHeaders(),
      responseType: 'blob'
    });
  }

  deleteFile(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/file/${id}`, { headers: this.getHeaders() });
  }

  // Folder operations
  getFolders(path: string = ''): Observable<FolderItem[]> {
    return this.http.get<FolderItem[]>(`${this.apiUrl}/folder?path=${encodeURIComponent(path)}`, { headers: this.getHeaders() });
  }

  getFiles(path: string = ''): Observable<FileItem[]> {
    return this.http.get<FileItem[]>(`${this.apiUrl}/folder/files?path=${encodeURIComponent(path)}`, { headers: this.getHeaders() });
  }

  getFolderById(id: string): Observable<FolderItemResponse> {
    return this.http.get<FolderItemResponse>(`${this.apiUrl}/folder/${id}`, { headers: this.getHeaders() });
  }

  getFolderContents(id: string): Observable<FolderContentsResult> {
    return this.http.get<FolderContentsResult>(`${this.apiUrl}/folder/${id}/contents`, { headers: this.getHeaders() });
  }

  createFolder(request: CreateFolderRequest): Observable<FolderItemResponse> {
    return this.http.post<FolderItemResponse>(`${this.apiUrl}/folder`, request, { headers: this.getHeaders() });
  }

  updateFolder(id: string, request: UpdateFolderRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/folder/${id}`, request, { headers: this.getHeaders() });
  }

  deleteFolder(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/folder/${id}`, { headers: this.getHeaders() });
  }

  moveFolder(id: string, destinationFolderId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/folder/${id}/move`, { destinationFolderId }, { headers: this.getHeaders() });
  }

  // Search operations
  search(query: SearchRequest): Observable<SearchResult> {
    const params = new URLSearchParams();
    params.set('q', query.q);
    if (query.type) params.set('type', query.type);
    if (query.page) params.set('page', query.page.toString());
    if (query.size) params.set('size', query.size.toString());
    
    return this.http.get<SearchResult>(`${this.apiUrl}/search?${params.toString()}`, { headers: this.getHeaders() });
  }

  getSearchSuggestions(q: string): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/search/suggestions?q=${encodeURIComponent(q)}`, { headers: this.getHeaders() });
  }
}
