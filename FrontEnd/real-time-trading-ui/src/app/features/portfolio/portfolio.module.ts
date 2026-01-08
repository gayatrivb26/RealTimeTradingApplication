import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PortfolioDashboardComponent } from './pages/portfolio-dashboard/portfolio-dashboard';
import { PortfolioRoutingModule } from './portfolio-routing.module';

@NgModule({
  declarations: [PortfolioDashboardComponent],
  imports: [
    CommonModule,
    PortfolioRoutingModule
  ]
})
export class PortfolioModule { }
