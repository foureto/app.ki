import { RouteObject } from "react-router-dom";
import MainPage from "../../pages/Main";
import LoginPage from "../../pages/Login";

export const publicRoutes: RouteObject[] = [
  { index: true, path: "/", element: <MainPage /> },
  { path: "/main", element: <MainPage /> },
  { path: "/login", element: <LoginPage /> },
];
