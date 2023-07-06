import { RouteObject, redirect } from "react-router-dom";
import PageLoader from "@components/PageLoader";
import SecretPage from "@pages/Secret";
import { IdentityService } from "@services/IdentityService";
import { ProtectedLayout } from "@layouts/ProtectedLayout";
import MainPage from "@pages/Main";
import { setUserRequested } from "../stores/app.store";

const children = [
  { path: "/main", element: <MainPage /> },
  { path: "*", element: <SecretPage /> },
];

export const protectedRoutes: RouteObject[] = [
  {
    path: "/",
    element: <ProtectedLayout />,
    errorElement: <PageLoader />,
    loader: async () => {
      try {
        const user = await IdentityService.me();
        if (user.data) {
          setUserRequested(user.data);
          return true;
        }
      } catch {
        return redirect("/login");
      }
      return redirect("/main");
    },
    children,
  },
];
