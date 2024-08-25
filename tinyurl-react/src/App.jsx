import {Route, Routes} from 'react-router-dom';
import Navbar from './components/Navbar/Navbar';
import HomePage from './pages/HomePage/HomePage';
import StatsPage from './pages/StatsPage/StatsPage.jsx';

import './App.css';

function App() {
    return (
        <>
            <Navbar/>
            <div className="app-container">
                <Routes>
                    <Route path="/"  element={<HomePage/>} />
                    <Route path="/stats"  element={<StatsPage />}/>
                </Routes>
            </div>
        </>
    );
}

export default App;
