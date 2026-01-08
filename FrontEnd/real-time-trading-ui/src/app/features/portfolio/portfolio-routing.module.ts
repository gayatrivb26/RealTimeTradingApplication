import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PortfolioDashboardComponent } from './pages/portfolio-dashboard/portfolio-dashboard';
import { AuthGuard } from '../../core/guard/auth-guard';

const routes: Routes = [
  {
    path: 'portfolio',
    component: PortfolioDashboardComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})

export class PortfolioRoutingModule { }

