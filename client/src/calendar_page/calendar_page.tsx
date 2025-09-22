import React, { useState } from 'react'
import { AddEvent } from './add_event.tsx'
import { EditEvent } from './edit_event.tsx'
import type { CalendarEvent } from '../calendar_event_client/calendar_event.ts'
import { ListEvents } from './list_events.tsx'

export const CalendarPage: React.FC = () => {
  const [events, setEvents] = useState<CalendarEvent[]>([])
  const [currentEvent, setCurrentEvent] = useState<CalendarEvent | null>(null)

  const handleAddEvent = (event: CalendarEvent) => {
    setEvents([...events, event])
  }

  const handleEditEvent = (updatedEvent: CalendarEvent) => {
    setEvents(
      events.map((event) =>
        event.id === updatedEvent.id ? updatedEvent : event
      )
    )
    setCurrentEvent(null) // Reset current event after editing
  }

  const handleCancelEvent = () => {
    setCurrentEvent(null) // Reset current event
  }

  return (
    <div>
      <h1>Calendar Events</h1>
      <h2>AddEvent</h2>
      <AddEvent onAdd={handleAddEvent} />
      <h2>EditEvent</h2>
      {currentEvent && (
        <EditEvent
          event={currentEvent}
          onEdit={handleEditEvent}
          onCancel={handleCancelEvent}
        />
      )}
      <h2>Events</h2>
      <ListEvents events={events} onEdit={setCurrentEvent} />
    </div>
  )
}
