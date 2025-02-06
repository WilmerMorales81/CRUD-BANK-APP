import { Navigate } from "react-router-dom";
import PropTypes from "prop-types";

export const AuthorizedRoute = ({ children, loggedInUser, roles, all }) => {
  let isAuthorized = false;

  if (loggedInUser) {
    if (roles && roles.length) {
      isAuthorized = all
        ? roles.every((role) => loggedInUser.roles.includes(role))
        : roles.some((role) => loggedInUser.roles.includes(role));
    } else {
      isAuthorized = true;
    }
  }

  return isAuthorized ? children : <Navigate to="/login" />;
};

AuthorizedRoute.propTypes = {
  children: PropTypes.node.isRequired,
  loggedInUser: PropTypes.object,
  roles: PropTypes.arrayOf(PropTypes.string),
  all: PropTypes.bool,
};
