import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})

export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onLogin() {
    this.authService.login(this.email, this.password)
      .subscribe({
        next: () => {
          this.router.navigate(['/portfolio']);
        },
        error: () => {
          this.errorMessage = 'Invallid email or password';
        }
      });
  }
}
