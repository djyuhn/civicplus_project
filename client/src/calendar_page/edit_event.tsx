import React from 'react'
import type { CalendarEvent } from '../calendar_event_client/calendar_event.ts'

interface EditEventProps {
  event: CalendarEvent | null
  onEdit: (event: CalendarEvent) => void
  onCancel: () => void
}

export const EditEvent: React.FC<EditEventProps> = ({
  event,
  onEdit,
  onCancel,
}) => {
  if (!event) return null

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const formData = new FormData(e.currentTarget)
    const updatedEvent: CalendarEvent = {
      ...event,
      title: formData.get('title') as string,
      description: formData.get('description') as string,
      startDate: new Date(formData.get('startDate') as string),
      endDate: new Date(formData.get('endDate') as string),
    }
    onEdit(updatedEvent)
  }
  const handleCancel = () => {
    onCancel()
  }

  return (
    <form onSubmit={handleSubmit}>
      <input type="text" name="title" defaultValue={event.title} required />
      <textarea name="description" defaultValue={event.description} required />
      <input
        type="datetime-local"
        name="startDate"
        defaultValue={event.startDate.toISOString().slice(0, 16)} // Format for input
        required
      />
      <input
        type="datetime-local"
        name="endDate"
        defaultValue={event.endDate.toISOString().slice(0, 16)} // Format for input
        required
      />
      <button type="button" onClick={handleCancel}>
        Cancel
      </button>
      <button type="submit">Edit Event</button>
    </form>
  )
}
