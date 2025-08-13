import { useEffect, useState } from "react";
import "./App.css";
import "./responsive.css";
import "bootstrap/dist/css/bootstrap.min.css";
import { tryGetLoggedInUser } from "./managers/authManager";
import { Spinner } from "reactstrap";
import NavBar from "./components/NavBar";
import ApplicationViews from "./components/ApplicationViews";

function App() {
  const [loggedInUser, setLoggedInUser] = useState(undefined);

  useEffect(() => {
    tryGetLoggedInUser()
      .then((user) => {
        setLoggedInUser(user);
      })
      .catch((error) => {
        console.error("Error fetching loggedInUser:", error);
        setLoggedInUser(null);
      });
  }, []);

  if (loggedInUser === undefined) {
    return (
      <div className="loading-container">
        <Spinner color="primary" />
        <p>Loading user information...</p>
      </div>
    );
  }

  if (!loggedInUser) {
    return (
      <div className="login-container">
        
        <ApplicationViews
          loggedInUser={loggedInUser}
          setLoggedInUser={setLoggedInUser}
        />
      </div>
    );
  }

  return (
    <>
      <NavBar loggedInUser={loggedInUser} setLoggedInUser={setLoggedInUser} />
      <ApplicationViews
        loggedInUser={loggedInUser}
        setLoggedInUser={setLoggedInUser}
      />
    </>
  );
}

export default App;
