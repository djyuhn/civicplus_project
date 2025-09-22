import React from 'react'
import type { CalendarEvent } from '../calendar_event_client/calendar_event.ts'

interface ListEventsProps {
  events: CalendarEvent[]
  onEdit: (event: CalendarEvent) => void
}

export const ListEvents: React.FC<ListEventsProps> = ({ events, onEdit }) => {
  return (
    <ul>
      {events.map((event) => (
        <li key={event.id}>
          <strong>{event.title}</strong> - {event.description} <br />
          {event.startDate.toLocaleString()} to {event.endDate.toLocaleString()}
          <button onClick={() => onEdit(event)}>Edit</button>
        </li>
      ))}
    </ul>
  )
}
