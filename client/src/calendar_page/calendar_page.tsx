import React, { useContext, useEffect, useState } from 'react'
import { AddEvent } from './add_event.tsx'
import { EditEvent } from './edit_event.tsx'
import type { CalendarEvent } from '../calendar_event_client/calendar_event.ts'
import { ListEvents } from './list_events.tsx'
import { CalendarEventClientContext } from '../calendar_event_client/calendar_event_client_context.tsx'

export const CalendarPage: React.FC = () => {
  const client = useContext(CalendarEventClientContext)
  const [events, setEvents] = useState<CalendarEvent[]>([])
  const [currentEvent, setCurrentEvent] = useState<CalendarEvent | null>(null)
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchEvents = async () => {
      if (client) {
        try {
          const response = await client.getCalendarEvents(0, 100)
          setEvents(response.items)
        } catch (err: unknown) {
          if (err instanceof Error) {
            setError(err.message)
          }
        } finally {
          setLoading(false)
        }
      }
    }

    fetchEvents().then()
  }, [client])

  const handleAddEvent = async (event: CalendarEvent) => {
    if (client) {
      try {
        const newEvent = await client.upsertCalendarEvent(event)
        setEvents((prevEvents) => [...prevEvents, newEvent])
      } catch (err: unknown) {
        if (err instanceof Error) {
          setError(err.message)
        }
      }
    }
  }

  const handleEditEvent = async (updatedEvent: CalendarEvent) => {
    if (client) {
      try {
        const editedEvent = await client.upsertCalendarEvent(updatedEvent)
        setEvents((prevEvents) =>
          prevEvents.map((event) =>
            event.id === editedEvent.id ? editedEvent : event
          )
        )
        setCurrentEvent(null) // Reset current event after editing
      } catch (err: unknown) {
        if (err instanceof Error) {
          setError(err.message)
        }
      }
    }
  }

  const handleCancelEvent = () => {
    setCurrentEvent(null) // Reset current event
  }

  if (loading) {
    return <div>Loading events...</div>
  }

  if (error) {
    return <div>Error: {error}</div>
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
