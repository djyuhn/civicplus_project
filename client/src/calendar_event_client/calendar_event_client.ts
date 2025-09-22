import type { CalendarEvent, GetCalendarEventsResponse, } from './calendar_event.ts'

interface calendarEventResponse {
  id: string
  title: string
  description: string
  startDate: Date
  endDate: Date
}

interface calendarEventsResponse {
  items: calendarEventResponse[]
  total: number
}

export class CalendarEventClient {
  private readonly baseUrl: string

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl
  }

  public async getCalendarEventById(id: string): Promise<CalendarEvent> {
    const response = await fetch(`${this.baseUrl}/api/Events/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error(response.statusText)
    }

    const data = (await response.json()) as calendarEventResponse

    return {
      id: data.id,
      title: data.title,
      description: data.description,
      startDate: new Date(data.startDate),
      endDate: new Date(data.endDate),
    }
  }

  public async getCalendarEvents(
    skip: number,
    top: number
  ): Promise<GetCalendarEventsResponse> {
    const response = await fetch(
      `${this.baseUrl}/api/Events?skip=${skip}&top=${top}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    )
    if (!response.ok) {
      throw new Error(response.statusText)
    }

    const data = (await response.json()) as calendarEventsResponse

    return {
      items: data.items.map((item) => {
        return {
          id: item.id,
          title: item.title,
          description: item.description,
          startDate: new Date(item.startDate),
          endDate: new Date(item.endDate),
        }
      }),
      total: data.total,
    }
  }

  public async upsertCalendarEvent(
    event: CalendarEvent
  ): Promise<CalendarEvent> {
    const response = await fetch(`${this.baseUrl}/api/Events`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(event),
    })
    if (!response.ok) {
      throw new Error(response.statusText)
    }

    const data = (await response.json()) as calendarEventResponse

    return {
      id: data.id,
      title: data.title,
      description: data.description,
      startDate: new Date(data.startDate),
      endDate: new Date(data.endDate),
    }
  }
}
