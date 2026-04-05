import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileItem, FolderItemResponse, FolderContentsResult, FileService, CreateFolderRequest } from '../../core/services/file.service';
import { Data, MyGlobalObject } from '../../core/services/MyGlobalObject';
import { AuthService } from '../../auth.service';
import { Router } from '@angular/router';

interface ExplorerItem {
  id: string;
  name: string;
  type: 'folder' | 'file';
  dateModified: string;
  size: string;
  raw: FileItem | FolderItemResponse;
}

@Component({
  selector: 'app-file-explorer-page',
  templateUrl: './file-explorer-page.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class FileExplorerPageComponent implements OnInit {
  view: 'details' | 'list' | 'icons' = 'details';
  selectedItems: ExplorerItem[] = [];
  currentPath: string = '';
  currentFolderId: string | null = null;
  activeTab: 'home' | 'share' | 'view' = 'home';
  showUserMenu: boolean = false;
  showNew: boolean = false;
  loading: boolean = false;
  error: string | null = null;

  files: ExplorerItem[] = [];

  constructor(
    public globalObject: MyGlobalObject,
    private authService: AuthService,
    private router: Router,
    private fileService: FileService
  ) {}

  ngOnInit(): void {
    this.loadFolderContents();
  }

  get userData(): Data {
    return this.globalObject.getUserData();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    const target = event.target as HTMLElement;
    const drowdown = target.closest('.user-drowdown');
    if (!drowdown) {
      this.showUserMenu = false;
    }
  }

  public logout() {
    this.authService.logout();
  }

  handleSelectItem(item: ExplorerItem): void {
    const index = this.selectedItems.findIndex((i) => i.id === item.id);
    if (index > -1) {
      this.selectedItems.splice(index, 1);
    } else {
      this.selectedItems.push(item);
    }
  }

  isSelected(item: ExplorerItem): boolean {
    return this.selectedItems.some((i) => i.id === item.id);
  }

  handleNavigateProfile(): void {
    this.router.navigate(['/profile']);
  }

  handleDoubleClickItem(item: ExplorerItem): void {
    if (item.type === 'folder') {
      this.currentFolderId = item.id;
      this.loadFolderContents();
    }
  }

  handleNavigateUp(): void {
    this.currentFolderId = null;
    this.currentPath = '';
    this.loadFolderContents();
  }

  fileById(index: number, item: ExplorerItem): string {
    return item.id;
  }

  setActiveTab(tab: 'home' | 'share' | 'view'): void {
    this.activeTab = tab;
  }

  setView(newView: 'details' | 'list' | 'icons'): void {
    this.view = newView;
  }

  loadFolderContents(): void {
    this.loading = true;
    this.error = null;

    if (this.currentFolderId) {
      this.fileService.getFolderContents(this.currentFolderId).subscribe({
        next: (result: FolderContentsResult) => {
          this.files = [
            ...result.folders.map(f => ({
              id: f.id,
              name: f.name,
              type: 'folder' as const,
              dateModified: new Date(f.createdAt).toLocaleDateString(),
              size: '-',
              raw: f
            })),
            ...result.files.map(f => ({
              id: f.id,
              name: f.name,
              type: 'file' as const,
              dateModified: new Date(f.modifiedAt).toLocaleDateString(),
              size: this.formatFileSize(f.size),
              raw: f
            }))
          ];
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error loading folder contents';
          this.loading = false;
          console.error('Error loading folder contents:', err);
        }
      });
    } else {
      // Load root folder contents
      this.fileService.getFolders(this.currentPath).subscribe({
        next: (folders) => {
          this.files = folders.map(f => ({
            id: f.id,
            name: f.name,
            type: 'folder' as const,
            dateModified: new Date(f.createdAt).toLocaleDateString(),
            size: '-',
            raw: f
          }));
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Error loading folders';
          this.loading = false;
          console.error('Error loading folders:', err);
        }
      });
    }
  }

  createNewFolder(): void {
    const folderName = prompt('Enter folder name:');
    if (folderName) {
      const request: CreateFolderRequest = {
        name: folderName,
        parentFolderId: this.currentFolderId
      };

      this.fileService.createFolder(request).subscribe({
        next: () => {
          this.loadFolderContents();
        },
        error: (err) => {
          alert('Error creating folder');
          console.error('Error creating folder:', err);
        }
      });
    }
  }

  deleteSelectedItems(): void {
    if (this.selectedItems.length === 0) return;

    if (confirm(`Are you sure you want to delete ${this.selectedItems.length} item(s)?`)) {
      let deleteCount = 0;
      const totalToDelete = this.selectedItems.length;

      this.selectedItems.forEach(item => {
        if (item.type === 'folder') {
          this.fileService.deleteFolder(item.id).subscribe({
            next: () => {
              deleteCount++;
              if (deleteCount === totalToDelete) {
                this.selectedItems = [];
                this.loadFolderContents();
              }
            },
            error: (err) => {
              console.error('Error deleting folder:', err);
              deleteCount++;
              if (deleteCount === totalToDelete) {
                this.selectedItems = [];
                this.loadFolderContents();
              }
            }
          });
        } else {
          this.fileService.deleteFile(item.id).subscribe({
            next: () => {
              deleteCount++;
              if (deleteCount === totalToDelete) {
                this.selectedItems = [];
                this.loadFolderContents();
              }
            },
            error: (err) => {
              console.error('Error deleting file:', err);
              deleteCount++;
              if (deleteCount === totalToDelete) {
                this.selectedItems = [];
                this.loadFolderContents();
              }
            }
          });
        }
      });
    }
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B';
    const k = 1024;
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  onFileUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.loading = true;

      this.fileService.uploadFile(file, this.currentFolderId || undefined).subscribe({
        next: (result) => {
          this.loading = false;
          this.loadFolderContents();
        },
        error: (err) => {
          this.loading = false;
          alert('Error uploading file');
          console.error('Error uploading file:', err);
        }
      });
    }
  }
}
