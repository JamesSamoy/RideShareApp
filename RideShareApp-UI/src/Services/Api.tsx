import axios from "axios";

const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL,
    headers: { "Content-Type": "application/json" },
});

type requestCodeByPhoneNumberRequest = {
    number: string;
}

type requestCodeByEmailRequest = {
    email: string;
}

type verifyCodeRequest = {
    code: string;
    contactInfo: string;
}

export const requestCodeByPhoneNumber = (request: requestCodeByPhoneNumberRequest) => api.post("/Login/PhoneNumber", request);
export const requestCodeByEmail = (request: requestCodeByEmailRequest) => api.post("/Login/Email", request);
export const verifyCode = (request: verifyCodeRequest) => api.post("/Login/VerifyCode", request);
