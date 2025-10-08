import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FileExplorerComponent } from './file-explorer.component'; // Importar el componente

@NgModule({
  declarations: [], // Los componentes Standalone no se declaran aquí
  imports: [
    CommonModule,
    FileExplorerComponent // Importar el componente standalone
  ],
  exports: [
    FileExplorerComponent // Exportar el componente standalone para que otros módulos/componentes puedan usarlo
  ]
})
export class FileExplorerModule { }