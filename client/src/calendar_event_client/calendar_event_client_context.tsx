import React, { createContext } from 'react'
import { CalendarEventClient } from './calendar_event_client.ts'
import config from '../../config.json'

export const CalendarEventClientContext =
  createContext<CalendarEventClient | null>(null)

export const CalendarEventClientProvider: React.FC<{
  children: React.ReactNode
}> = ({ children }) => {
  const url = `${config.calendarEventApiUrl}:${config.calendarEventApiPort}`
  const client = new CalendarEventClient(url)

  return (
    <CalendarEventClientContext.Provider value={client}>
      {children}
    </CalendarEventClientContext.Provider>
  )
}
