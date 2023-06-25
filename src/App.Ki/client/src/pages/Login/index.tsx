import React from "react";
import { Navigate } from "react-router-dom";
import { useStore } from "effector-react";
import { $data, loginOneRequested, loginTwoRequested } from "./login.store";
import { $userStore } from "../../commons/stores/app.store";

import "./login.scss";
import PageLoader from "../../components/PageLoader";

const LoginPage: React.FC = () => {
  const user = useStore($userStore);
  const { loading, step } = useStore($data);

  if (user) return <Navigate to={"/main"} />;

  if (loading) return <PageLoader />;
  console.log(step);

  const content =
    step === 0 ? (
      <button onClick={() => loginOneRequested()}>login</button>
    ) : step === 1 ? (
      <button onClick={() => loginTwoRequested({ code: "123" })}>
        confirm
      </button>
    ) : (
      <Navigate to={"/main"} />
    );

  return (
    <div className="login-layout">
      <div className="login-container">{content}</div>
      <div className="text-small">terms of use goes here...</div>
    </div>
  );
};

export default LoginPage;
