import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { HomeComponent } from './home/home.component';
import { redirectIfLoggedInGuard } from './auth/guard';
import { SessionsComponent } from './sessions/sessions.component';
import { ProfileComponent } from './profile/profile.component';

export const routes: Routes = [
    {
        path: '', redirectTo: 'login',  pathMatch: 'full'
    },
    {
        path: 'login', component: LoginComponent, canActivate: [redirectIfLoggedInGuard]
    },
    {
        path: 'signup', component: SignupComponent, canActivate: [redirectIfLoggedInGuard]
    },
    {
        path: 'home', component: HomeComponent
    },
    {
        path: 'sessions', component: SessionsComponent
    },
    {
        path: 'profile', component: ProfileComponent
    },
    {
        path: '**', redirectTo: 'login'
    },


];
