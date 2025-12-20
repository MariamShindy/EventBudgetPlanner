//Request models 
export interface LoginRequest {
    email: string;
    password: string;
}
export interface RegisterRequest {
    fullName: string;
    email: string;
    password: string;
    confirmPassword: string;    
}
export interface ForgotPasswordRequest {
    email: string;
}
export interface ResetPasswordRequest {
    email: string;
    token: string;
    newPassword: string;
    confirmPassword: string;    
}
export interface UpdateUserRequest {
    fullName?: string;
    email?: string;
}

//Response models
export interface AuthResponse {
    userId: string;
    fullName: string;
    email: string;
    token: string;
    expiresAt: string;
}
export interface UserInfo {
    userId: string;
    email: string;
    fullName: string;
}
export interface TokenData{
    token : string;
    expiresAt : string;
    userId : string;
}
