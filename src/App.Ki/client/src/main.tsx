import React from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { publicRoutes } from "./commons/routes/public";
import { protectedRoutes } from "./commons/routes/protected";
import "./index.scss";

const rootElement = document.getElementById("root");
const root = createRoot(rootElement as HTMLElement);

const router = createBrowserRouter([...publicRoutes, ...protectedRoutes]);

root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);
