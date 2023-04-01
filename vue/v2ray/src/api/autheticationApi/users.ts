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

export const getUser = () =>
  request({
    url: "/authentication/get-current-user",
    method: "post",
  });
export const register = (data: any) =>
  request({
    url: "/authentication/register",
    method: "post",
    data,
  });
export const getCode = (input) =>
  request({
    url: `/authentication/get-code/${input.mobile}/${input.loginToken}`,
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
