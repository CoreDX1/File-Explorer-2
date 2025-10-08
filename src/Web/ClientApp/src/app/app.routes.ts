import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { FileExplorerPageComponent } from './features/file-explorer-page/file-explorer-page.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'explorer', 
    component: FileExplorerPageComponent,
    canActivate: [AuthGuard]
  },
  {
    path: '', 
    redirectTo: '/login', 
    pathMatch: 'full'
  }
];