import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import { BrowserRouter, Route, Routes } from "react-router-dom"
import About from "./Pages/About.tsx"
import RootLayout from "./Layouts/RootLayout.tsx"

createRoot(document.getElementById('root')!).render(
  <StrictMode>
      <BrowserRouter>
          <Routes>
              <Route path="/" element={
                  <RootLayout>
                      <App />
                  </RootLayout>
              } />
              <Route path="/about" element={<About />} />
          </Routes>
      </BrowserRouter>
  </StrictMode>
)
