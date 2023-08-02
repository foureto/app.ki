import { useStore } from "effector-react";
import React from "react";
import { Navigate, useOutlet } from "react-router-dom";
import { $userStore } from "../commons/stores/app.store";

import "./ProtectedLayout.scss";
import Header from "./Header";
import Footer from "./Footer";
import SideBar from "./SideBar";

export const ProtectedLayout: React.FC = () => {
  const user = useStore($userStore);
  const outlet = useOutlet();

  if (!user) {
    return <Navigate to="/login" />;
  }

  return (
    <div className="app-container">
      <SideBar />
      <Header />
      <main className="main">
        <section className="main-content">
          <div className="main-container">{outlet}</div>
        </section>
      </main>
      <Footer>footer</Footer>
    </div>
  );
};
