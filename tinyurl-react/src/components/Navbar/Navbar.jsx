import { Link, NavLink } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => (
    <nav className="navbar">
        <div className="navbar-brand">
            <Link to="/">TinyURL-style service</Link>
        </div>
        <ul className="navbar-links">
            <li>
                <NavLink exact="true" to="/"
                         className={({ isActive}) =>
                             [
                                 isActive ? "active" : "",
                             ].join(" ")
                         }>
                    Home
                </NavLink>
            </li>
            <li>
                <NavLink to="/stats"
                         className={({ isActive}) =>
                             [
                                 isActive ? "active" : "",
                             ].join(" ")
                         }>
                    Stats
                </NavLink>
            </li>
        </ul>
    </nav>
);

export default Navbar;
