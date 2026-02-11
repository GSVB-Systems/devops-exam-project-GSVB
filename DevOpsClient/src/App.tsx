import './App.css'
import Login from './components/Login'
import EggSnapshotCard from './components/EggSnapshotCard'

function App() {
  return (
    <div className="app-root min-h-screen bg-base-200 py-10 flex items-center justify-center">
      <div className="app-inner w-full max-w-4xl px-4 space-y-6">
        <EggSnapshotCard />
        <Login />
      </div>
    </div>
  )
}

export default App
