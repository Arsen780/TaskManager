import api from "./axiosConfig";
import type { LoginRequest, LoginResponse } from "../types/login";

export const loginUser = async(
    data: LoginRequest
): Promise<LoginResponse> => {
    const response = await api.post<LoginResponse>("Login",data);
    return response.data;
}