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

/// \file
/// \ingroup qf
/// \brief QTimeEvt::ctr() implementation.

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif

//............................................................................
QTimeEvtCtr QTimeEvt::ctr(void) {
    QTimeEvtCtr ctr;
    QF_CRIT_STAT_
    QF_CRIT_ENTRY_();
    if (m_prev != (QTimeEvt *)0) {        // is the time event actually armed?
        ctr = m_ctr;
    }
    else {                                     // the time event was not armed
        ctr = (QTimeEvtCtr)0;
    }

    QS_BEGIN_NOCRIT_(QS_QF_TIMEEVT_CTR, QS::teObj_, this)
        QS_TIME_();                                               // timestamp
        QS_OBJ_(this);                               // this time event object
        QS_OBJ_(m_act);                                   // the active object
        QS_TEC_(ctr);                                   // the current counter
        QS_TEC_(m_interval);                                   // the interval
    QS_END_NOCRIT_()

    QF_CRIT_EXIT_();
    return ctr;
}

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif
