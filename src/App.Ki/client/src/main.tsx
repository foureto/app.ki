import React from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider, createBrowserRouter } from "react-router-dom";
import { ConfigProvider } from "antd";
import { publicRoutes } from "./commons/routes/public";
import { protectedRoutes } from "./commons/routes/protected";
import "./index.scss";
import theme from "./theme";

const rootElement = document.getElementById("root");
const root = createRoot(rootElement as HTMLElement);

const router = createBrowserRouter([...publicRoutes, ...protectedRoutes]);

root.render(
  <React.StrictMode>
    <ConfigProvider theme={theme}>
      <RouterProvider router={router} />
    </ConfigProvider>
  </React.StrictMode>
);
