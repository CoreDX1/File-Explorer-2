import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileItem } from '../../core/models/file-item.model';
import { Data, MyGlobalObject } from '../../core/services/MyGlobalObject';
import { AuthService } from '../../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-file-explorer-page',
  templateUrl: './file-explorer-page.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class FileExplorerPageComponent {
  view: 'details' | 'list' | 'icons' = 'details';
  selectedItems: FileItem[] = [];
  currentPath: string = 'C:\\Users\\User\\Documents';
  activeTab: 'home' | 'share' | 'view' = 'home';
  showUserMenu: boolean = false;

  files: FileItem[] = [
    {
      id: 1,
      name: 'Project Files',
      type: 'folder',
      dateModified: '2023-06-15',
      size: '-',
    },
    {
      id: 2,
      name: 'Report.pdf',
      type: 'file',
      dateModified: '2023-06-10',
      size: '2.4 MB',
    },
    {
      id: 3,
      name: 'Photos',
      type: 'folder',
      dateModified: '2023-06-05',
      size: '-',
    },
    {
      id: 4,
      name: 'Budget.xlsx',
      type: 'file',
      dateModified: '2023-06-14',
      size: '345 KB',
    },
    {
      id: 5,
      name: 'Personal',
      type: 'folder',
      dateModified: '2023-05-30',
      size: '-',
    },
    {
      id: 6,
      name: 'Resume.docx',
      type: 'file',
      dateModified: '2023-06-12',
      size: '189 KB',
    },
    {
      id: 7,
      name: 'Downloads',
      type: 'folder',
      dateModified: '2023-06-16',
      size: '-',
    },
    {
      id: 8,
      name: 'Invoices',
      type: 'folder',
      dateModified: '2023-06-08',
      size: '-',
    },
    {
      id: 9,
      name: 'Presentation.pptx',
      type: 'file',
      dateModified: '2023-06-11',
      size: '5.2 MB',
    },
    {
      id: 10,
      name: 'Designs',
      type: 'folder',
      dateModified: '2023-06-15',
      size: '-',
    },
    {
      id: 11,
      name: 'Notes.txt',
      type: 'file',
      dateModified: '2023-06-13',
      size: '12 KB',
    },
    {
      id: 12,
      name: 'Archive',
      type: 'folder',
      dateModified: '2023-06-09',
      size: '-',
    },
  ];

  constructor(
    public globalObject: MyGlobalObject,
    private authService: AuthService,
    private router: Router
  ) {}

  get userData(): Data {
    // console.log("Fetching user name e from global object");
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

  handleSelectItem(item: FileItem): void {
    const index = this.selectedItems.findIndex((i) => i.id === item.id);
    if (index > -1) {
      this.selectedItems.splice(index, 1);
    } else {
      this.selectedItems.push(item);
    }
  }

  isSelected(item: FileItem): boolean {
    return this.selectedItems.some((i) => i.id === item.id);
  }

  handleNavigateProfile(): void {
    console.log('Navegando a profile...');
    this.router.navigate(['/profile']).then(
      (success) => console.log('Navegación exitosa:', success),
      (error) => console.log('Error en navegación:', error)
    );
  }

  handleDoubleClickItem(item: FileItem): void {
    if (item.type === 'folder') {
      this.currentPath = `${this.currentPath}\\${item.name}`;
      this.selectedItems = [];
    }
  }

  fileById(index: number, item: FileItem): number {
    return item.id;
  }

  setActiveTab(tab: 'home' | 'share' | 'view'): void {
    this.activeTab = tab;
  }

  setView(newView: 'details' | 'list' | 'icons'): void {
    this.view = newView;
  }
}
