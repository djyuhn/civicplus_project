import './App.css'
import { CalendarPage } from './calendar_page/calendar_page.tsx'
import { CalendarEventClientProvider } from './calendar_event_client/calendar_event_client_context.tsx'

function App() {
  return (
    <CalendarEventClientProvider>
      <CalendarPage />
    </CalendarEventClientProvider>
  )
}

export default App
