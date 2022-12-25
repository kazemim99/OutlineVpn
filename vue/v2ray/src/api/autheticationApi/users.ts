import request from "@/utils/request";
import {
  IApiResponse,
  ILoginResponse,
  ILoginResultDto,
} from "./model/ILoginResponse";

export const login = (data: any) =>
  request({
    url: "/authentication/login",
    method: "post",
    data,
  });

export const register = (data: any) =>
  request({
    url: "/authentication/register",
    method: "post",
    data,
  });
export const getCode = (mobile: string) =>
  request({
    url: `/authentication/get-code/${mobile}`,
    method: "get",
  });

export const veriFyCode = (data: any) =>
  request({
    url: `/authentication/verify-code`,
    method: "post",
    data,
  });

export const changePassword = (
  mobile: string,
  data: { password: string; confirmPassword: string }
) =>
  request({
    url: `/authentication/change-password/${mobile}`,
    method: "put",
    data,
  });
