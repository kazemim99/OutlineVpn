import Cookies from "js-cookie";

// User
const tokenKey = "X-Access-Token";
export const getToken = () => Cookies.get(tokenKey);
export const setToken = (token: string) => Cookies.set(tokenKey, token, {
  expires: 7,  // Cookie expires in 7 days
  path: '/',   // Available across the entire domain
  sameSite: 'lax' // Allows cookie to be sent with top-level navigation
});
export const removeToken = () => Cookies.remove(tokenKey);
