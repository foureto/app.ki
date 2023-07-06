import { RouteObject } from "react-router-dom";
import LoginPage from "@pages/Login";

export const publicRoutes: RouteObject[] = [
  { index: true, path: "/", element: <LoginPage /> },
  { path: "/login", element: <LoginPage /> },
];
