import { Route, Routes } from "react-router-dom";
import PropTypes from "prop-types";
import Accounts from "./Accounts/Accounts";
import { AuthorizedRoute } from "./auth/AuthorizedRoute";
import Login from "./auth/Login";
import Register from "./auth/Register";
import Payments from "./Payments/Payments";
import EditCustomer from "./customers/EditProfile";
import NewAccount from "./newAccount/NewAccount";

export default function ApplicationViews({ loggedInUser, setLoggedInUser }) {
  return (
    <Routes>
      <Route path="/">
        <Route
          index
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <Accounts loggedInUser={loggedInUser} />
            </AuthorizedRoute>
          }
        />
        <Route
          path="accounts"
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <Accounts loggedInUser={loggedInUser} />
            </AuthorizedRoute>
          }
        />
        <Route
          path="customers/:id"
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <EditCustomer />
            </AuthorizedRoute>
          }
        />
        <Route
          path="Payments"
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <Payments loggedInUser={loggedInUser} />
            </AuthorizedRoute>
          }
        />
        <Route
          path="newAccount"
          element={
            <AuthorizedRoute loggedInUser={loggedInUser}>
              <NewAccount loggedInUser={loggedInUser} />
            </AuthorizedRoute>
          }
        />
        <Route
          path="customers"
          element={
            <AuthorizedRoute roles={["Admin"]} loggedInUser={loggedInUser}>
              <p>Employees</p>
            </AuthorizedRoute>
          }
        />
        <Route
          path="login"
          element={<Login setLoggedInUser={setLoggedInUser} />}
        />
        <Route
          path="register"
          element={<Register setLoggedInUser={setLoggedInUser} />}
        />
      </Route>
      <Route path="*" element={<p>Whoops, nothing here...</p>} />
    </Routes>
  );
}

ApplicationViews.propTypes = {
  loggedInUser: PropTypes.shape({
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
    roles: PropTypes.arrayOf(PropTypes.string),
    profile: PropTypes.object,
  }),
  setLoggedInUser: PropTypes.func.isRequired,
};
