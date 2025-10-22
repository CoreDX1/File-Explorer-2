import { Component, Output, EventEmitter, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FileItem } from '../../core/models/file-item.model';

@Component({
  selector: 'app-file-explorer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './file-explorer.component.html',
})
export class FileExplorerComponent {
  @Input() view: 'details' | 'list' | 'icons' = 'details';
  @Output() pathChanged = new EventEmitter<string>();
  @Output() selectionChanged = new EventEmitter<number>();
  selectedItems: FileItem[] = [];
  currentPath: string = 'C:\\Users\\User\\Documents';

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

  constructor() {
    this.pathChanged.emit(this.currentPath);
  }

  handleSelectItem(item: FileItem): void {
    const index = this.selectedItems.findIndex((i) => i.id === item.id);
    if (index > -1) {
      this.selectedItems.splice(index, 1);
    } else {
      this.selectedItems.push(item);
    }
    this.selectionChanged.emit(this.selectedItems.length);
  }

  isSelected(item: FileItem): boolean {
    return this.selectedItems.some((i) => i.id === item.id);
  }

  handleDoubleClickItem(item: FileItem): void {
    if (item.type === 'folder') {
      this.currentPath = `${this.currentPath}\\${item.name}`;
      this.pathChanged.emit(this.currentPath);
      this.selectedItems = [];
    }
  }

  fileById(index: number, item: FileItem): number {
    return item.id;
  }
}
