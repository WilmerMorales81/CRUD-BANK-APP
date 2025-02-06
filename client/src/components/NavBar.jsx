import { useState } from "react";
import { NavLink as RRNavLink } from "react-router-dom";
import {
  Button,
  Collapse,
  Nav,
  NavLink,
  NavItem,
  Navbar,
  NavbarBrand,
  NavbarToggler,
} from "reactstrap";
import { logout } from "../managers/authManager";

// eslint-disable-next-line react/prop-types
export default function NavBar({ loggedInUser, setLoggedInUser }) {
  const [open, setOpen] = useState(false);

  const toggleNavbar = () => setOpen(!open);

  const handleLogout = async (e) => {
    e.preventDefault();
    await logout(); // Llamar la función logout de `authManager.js`
    setLoggedInUser(null); // Limpiar el estado del usuario
  };

  return (
    <div>
      <Navbar color="success" dark fixed="true" expand="lg">
        <NavbarBrand className="mr-auto" tag={RRNavLink} to="/">
          <img
            src="./CrudBankLogo.png"
            alt="CrudBank"
            height={50}
            style={{ marginRight: "8px" }}
          />
        </NavbarBrand>
        {loggedInUser ? (
          <>
            <NavbarToggler onClick={toggleNavbar} />
            <Collapse isOpen={open} navbar>
              <Nav navbar>
                <NavItem onClick={() => setOpen(false)}>
                  <NavLink tag={RRNavLink} to="/accounts">
                    MyProducts
                  </NavLink>
                </NavItem>
                <NavItem onClick={() => setOpen(false)}>
                  <NavLink tag={RRNavLink} to="/Payments">
                    Payments
                  </NavLink>
                </NavItem>
                <NavItem onClick={() => setOpen(false)}>
                  <NavLink tag={RRNavLink} to="/newAccount">
                    New Product
                  </NavLink>
                </NavItem>
              </Nav>
            </Collapse>
            <Button color="dark" outline onClick={handleLogout}>
              Logout
            </Button>
          </>
        ) : (
          <Nav navbar>
            <NavItem>
              <NavLink tag={RRNavLink} to="/login">
                <Button color="dark" outline>
                  Login
                </Button>
              </NavLink>
            </NavItem>
          </Nav>
        )}
      </Navbar>
    </div>
  );
}
