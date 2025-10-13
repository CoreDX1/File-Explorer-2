import { Routes } from '@angular/router';
import { LoginComponent } from './features/login/login.component';
import { FileExplorerPageComponent } from './features/file-explorer-page/file-explorer-page.component';
import { AdminDashboardComponent } from './features/admin-dashboard/admin-dashboard.component';
import { StoragePaymentComponent } from './features/storage-payment/storage-payment.component';
import { UserProfileComponent } from './features/user-profile/user-profile.component';
import { ApplicationSettingsComponent } from './features/application-settings/application-settings.component';
import { AdminUsersComponent } from './features/admin-users/admin-users.component';
import { GroupsManagementComponent } from './features/groups-management/groups-management.component';
import { FileSharingComponent } from './features/file-sharing/file-sharing.component';
import { ActivityHistoryComponent } from './features/activity-history/activity-history.component';
import { TrashBinComponent } from './features/trash-bin/trash-bin.component';
import { AdvancedSearchComponent } from './features/advanced-search/advanced-search.component';
import { NotificationsComponent } from './features/notifications/notifications.component';
import { HelpDocumentationComponent } from './features/help-documentation/help-documentation.component';
import { AdminReportsComponent } from './features/admin-reports/admin-reports.component';
import { BillingPaymentsComponent } from './features/billing-payments/billing-payments.component';
import { SecuritySettingsComponent } from './features/security-settings/security-settings.component';
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
    path: 'admin', 
    component: AdminDashboardComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'storage', 
    component: StoragePaymentComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'profile', 
    component: UserProfileComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'settings', 
    component: ApplicationSettingsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'admin/users', 
    component: AdminUsersComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'groups', 
    component: GroupsManagementComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'sharing', 
    component: FileSharingComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'activity', 
    component: ActivityHistoryComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'trash', 
    component: TrashBinComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'search', 
    component: AdvancedSearchComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'notifications', 
    component: NotificationsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'help', 
    component: HelpDocumentationComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'admin/reports', 
    component: AdminReportsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'billing', 
    component: BillingPaymentsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'security', 
    component: SecuritySettingsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: '', 
    redirectTo: '/login', 
    pathMatch: 'full'
  }
];