import { useStore } from "effector-react";
import React from "react";
import { Navigate, useOutlet } from "react-router-dom";
import { $userStore } from "../commons/stores/app.store";

import "./ProtectedLayout.scss";
import Header from "../components/Header";
import Footer from "../components/Footer";
import SideBar from "../components/SideBar";

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
        <section className="main-content">{outlet}</section>
      </main>
      <Footer>footer</Footer>
    </div>
  );
};
