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
      <Navbar color="success" dark fixed="true" expand="lg" className="py-2">
        <NavbarBrand className="mr-auto" tag={RRNavLink} to="/">
          <img
            src="./CrudBankLogo.png"
            alt="CrudBank"
            height={50}
            style={{ marginRight: "8px" }}
            className="d-none d-sm-inline"
          />
          <img
            src="./CrudBankLogo.png"
            alt="CrudBank"
            height={40}
            style={{ marginRight: "8px" }}
            className="d-inline d-sm-none"
          />
        </NavbarBrand>
        {loggedInUser ? (
          <>
            <NavbarToggler onClick={toggleNavbar} className="border-0" />
            <Collapse isOpen={open} navbar>
              <Nav navbar className="w-100">
                <NavItem onClick={() => setOpen(false)} className="w-100">
                  <NavLink tag={RRNavLink} to="/accounts" className="text-center py-3">
                    MyProducts
                  </NavLink>
                </NavItem>
                <NavItem onClick={() => setOpen(false)} className="w-100">
                  <NavLink tag={RRNavLink} to="/Payments" className="text-center py-3">
                    Payments
                  </NavLink>
                </NavItem>
                <NavItem onClick={() => setOpen(false)} className="w-100">
                  <NavLink tag={RRNavLink} to="/newAccount" className="text-center py-3">
                    New Product
                  </NavLink>
                </NavItem>
              </Nav>
            </Collapse>
            <Button color="dark" outline onClick={handleLogout} className="ms-2">
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
