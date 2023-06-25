import { useStore } from "effector-react";
import React from "react";
import { $userStore } from "../../commons/stores/app.store";

const Header: React.FC = () => {
  const user = useStore($userStore);
  return (
    <div className="header">
      Hi, <a href="#">{user?.name}</a> | <a href="#">Logout</a>
    </div>
  );
};

export default Header;
