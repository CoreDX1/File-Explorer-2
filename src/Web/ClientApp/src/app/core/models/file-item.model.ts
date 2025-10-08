export interface FileItem {
  id: number;
  name: string;
  type: 'folder' | 'file';
  dateModified: string;
  size: string;
}
