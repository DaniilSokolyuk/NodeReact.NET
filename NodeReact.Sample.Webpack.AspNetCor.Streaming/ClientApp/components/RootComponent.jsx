import React, { lazy, Suspense } from "react";
import { Link, BrowserRouter, Route, Routes, Navigate } from "react-router-dom";
import { StaticRouter } from "react-router-dom/server";
const LoremIpsum = lazy(() => import("./LoremIpsum"));

const Navbar = () => (
  <ul>
    <li>
      <Link to="/">Home</Link>
    </li>

    <li>
      <Link to="/about">About</Link>
    </li>

    <li>
      <Link to="/contact">Contact</Link>
    </li>

    <li>
      <Link to="/lorem">Lorem</Link>
    </li>
  </ul>
);

const HomePage = () => <h1>Home</h1>;

const AboutPage = () => <h1>About</h1>;

const ContactPage = () => <h1>Contact</h1>;

const SSRContext = React.createContext({});

const WithSSRContext = ({ status, location, children }) => {
  const SSRContext_ = React.useContext(SSRContext);

  SSRContext_.status = status;
  SSRContext_.url = location;

  return children;
};

const Redirect = ({ location }) => (
  <WithSSRContext status={302} location={location}>
    {null}
  </WithSSRContext>
);

const RootComponent = ({ location, context }) => {
  const app = (
    <div>
      <Navbar />

      <Routes>
        <Route path="/" element={<Redirect location="/home" />} />

        <Route path="/home" element={<HomePage />} />

        <Route path="/about" element={<AboutPage />} />

        <Route path="/contact" element={<ContactPage />} />

        <Route
          path="/lorem"
          element={
            <Suspense fallback={<div>Loading...</div>}>
              <LoremIpsum />
            </Suspense>
          }
        />

        <Route
          path="*"
          element={
            <WithSSRContext status={404}>
              <h1>Not Found :(</h1>
            </WithSSRContext>
          }
        />
      </Routes>
    </div>
  );

  if (typeof window === "undefined") {
    return (
      <SSRContext.Provider value={context}>
        <StaticRouter location={location}>{app}</StaticRouter>
      </SSRContext.Provider>
    );
  }

  return <BrowserRouter>{app}</BrowserRouter>;
};

export default RootComponent;
