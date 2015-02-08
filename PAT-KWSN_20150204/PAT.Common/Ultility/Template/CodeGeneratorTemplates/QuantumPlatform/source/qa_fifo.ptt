//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 01, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#include "qf_pkg.h"
#include "qassert.h"

/// \file
/// \ingroup qf
/// \brief QActive::postFIFO() implementation.
///
/// \note this source file is only included in the QF library when the native
/// QF active object queue is used (instead of a message queue of an RTOS).

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

Q_DEFINE_THIS_MODULE(qa_fifo)

//............................................................................
#ifndef Q_SPY
void QActive::postFIFO(QEvent const *e) {
#else
void QActive::postFIFO(QEvent const *e, void const *sender) {
#endif

    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();

    QS_BEGIN_NOCRIT_(QS_QF_ACTIVE_POST_FIFO, QS::aoObj_, this)
        QS_TIME_();                                               // timestamp
        QS_OBJ_(sender);                                  // the sender object
        QS_SIG_(e->sig);                            // the signal of the event
        QS_OBJ_(this);                                   // this active object
        QS_U8_(EVT_POOL_ID(e));                    // the pool Id of the event
        QS_U8_(EVT_REF_CTR(e));                  // the ref count of the event
        QS_EQC_(m_eQueue.m_nFree);                   // number of free entries
        QS_EQC_(m_eQueue.m_nMin);                // min number of free entries
    QS_END_NOCRIT_()

    if (EVT_POOL_ID(e) != (uint8_t)0) {              // is it a dynamic event?
        EVT_INC_REF_CTR(e);                 // increment the reference counter
    }

    if (m_eQueue.m_frontEvt == (QEvent *)0) {           // is the queue empty?
        m_eQueue.m_frontEvt = e;                     // deliver event directly
        QACTIVE_EQUEUE_SIGNAL_(this);                // signal the event queue
    }
    else {               // queue is not empty, leave event in the ring-buffer
                                        // queue must accept all posted events
        Q_ASSERT(m_eQueue.m_nFree != (QEQueueCtr)0);
        m_eQueue.m_ring[m_eQueue.m_head] = e;//insert e into the buffer (FIFO)
        if (m_eQueue.m_head == (QEQueueCtr)0) {      // need to wrap the head?
            m_eQueue.m_head = m_eQueue.m_end;                   // wrap around
        }
        --m_eQueue.m_head;

        --m_eQueue.m_nFree;                    // update number of free events
        if (m_eQueue.m_nMin > m_eQueue.m_nFree) {
            m_eQueue.m_nMin = m_eQueue.m_nFree;       // update minimum so far
        }
    }
    QF_CRIT_EXIT_();
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif
