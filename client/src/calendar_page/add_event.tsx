import React from 'react'
import type { CalendarEvent } from '../calendar_event_client/calendar_event.ts'

interface AddEventProps {
  onAdd: (event: CalendarEvent) => void
}

export const AddEvent: React.FC<AddEventProps> = ({ onAdd }) => {
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const newEvent: CalendarEvent = {
      id: Date.now().toString(),
      title: formData.get('title') as string,
      description: formData.get('description') as string,
      startDate: new Date(formData.get('startDate') as string),
      endDate: new Date(formData.get('endDate') as string),
    }
    onAdd(newEvent)
    e.currentTarget.reset() // Reset the form after submission
  }

  return (
    <form onSubmit={handleSubmit}>
      <input type="text" name="title" placeholder="Event Title" required />
      <textarea name="description" placeholder="Event Description" required />
      <input type="datetime-local" name="startDate" required />
      <input type="datetime-local" name="endDate" required />
      <button type="submit">Add Event</button>
    </form>
  )
}
